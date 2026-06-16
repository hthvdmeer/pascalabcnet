//winonly
begin
  var f: System.Func<byte> := ()->byte(2);
  var t := System.Threading.Tasks.Task.Run(f);
  t.Wait();
  assert(t.Result = 2);
end.