using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.FrameworkTest
{
    internal class EmitTest
    {
        public static void Test()
        {
            // Create a dynamic method with the specified signature.
            DynamicMethod dm = new DynamicMethod("SetIdToNull", null, new[] { typeof(DrUserDto) });

            // Get the ILGenerator for the dynamic method.
            ILGenerator il = dm.GetILGenerator();

            LocalBuilder result = il.DeclareLocal(typeof(DrUserDto));
            il.Emit(OpCodes.Newobj, typeof(DrUserDto).GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, result);

            LocalBuilder x = il.DeclareLocal(typeof(int?));
            il.Emit(OpCodes.Ldloca_S, x);   // 加载值类型地址
            il.Emit(OpCodes.Initobj, typeof(int?)); // 初始化为null

            // Emit the IL code.
            il.Emit(OpCodes.Ldloc, result);  // Load the 'drUserDto' object onto the stack.
            il.Emit(OpCodes.Ldloca_S, x);   // 加载x地址
            il.Emit(OpCodes.Ldobj, typeof(int?));  // 加载x 值
            il.Emit(OpCodes.Callvirt, typeof(DrUserDto).GetProperty("Id").GetSetMethod());  // Call the setter of the 'Id' property.
            il.Emit(OpCodes.Nop);  // Emit a nop instruction (optional).
            il.Emit(OpCodes.Nop);  // Emit another nop instruction (optional).
            il.Emit(OpCodes.Ret);  // Return from the method.

            // Create a delegate for the dynamic method.
            Action<DrUserDto> setIdToNullDelegate = (Action<DrUserDto>)dm.CreateDelegate(typeof(Action<DrUserDto>));

            // Test the delegate.
            DrUserDto drUserDto = new DrUserDto { Id = 123 };
            setIdToNullDelegate(drUserDto);

            Console.WriteLine($"Id after setting to null: {drUserDto.Id}");
        }
    }
}
